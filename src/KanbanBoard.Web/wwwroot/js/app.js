const API_BASE = window.location.origin;
let currentBoardId = null;
const MAX_RECENT_BOARDS = 10;

document.addEventListener('DOMContentLoaded', () => {
    initializeApp();
});

function initializeApp() {
    document.getElementById('createTaskBtn').addEventListener('click', () => openTaskModal());
    document.getElementById('refreshBtn').addEventListener('click', () => loadTasks());
    document.getElementById('generateBoardBtn').addEventListener('click', generateNewBoard);
    document.getElementById('loadBoardBtn').addEventListener('click', loadBoard);
    document.getElementById('taskForm').addEventListener('submit', handleTaskSubmit);
    document.getElementById('cancelBtn').addEventListener('click', closeTaskModal);
    document.querySelector('.close').addEventListener('click', closeTaskModal);

    document.getElementById('boardNameInput').addEventListener('blur', saveBoardName);
    document.getElementById('boardNameInput').addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            saveBoardName();
        }
    });

    initializeSortable();

    const savedBoardId = localStorage.getItem('kanbanBoardId');
    if (savedBoardId) {
        currentBoardId = savedBoardId;
        loadBoardDetails(savedBoardId);
        updateRecentBoards(savedBoardId);
        loadTasks();
    } else {
        generateNewBoard();
    }
}

function generateNewBoard() {
    const newBoardId = crypto.randomUUID();
    const defaultName = `Board ${new Date().toLocaleDateString()}`;

    document.getElementById('boardIdInput').value = newBoardId;
    document.getElementById('boardNameInput').value = defaultName;

    currentBoardId = newBoardId;
    saveBoardName(defaultName);
    saveCurrentBoard(newBoardId);
    clearBoard();
    showNotification('New board created! Give it a name.', 'success');
}

function loadBoard() {
    const boardId = document.getElementById('boardIdInput').value.trim();
    if (!boardId) {
        showNotification('Please enter a valid Board ID', 'error');
        return;
    }
    currentBoardId = boardId;
    loadBoardDetails(boardId);
    saveCurrentBoard(boardId);
    loadTasks();
}

function loadBoardDetails(boardId) {
    document.getElementById('boardIdInput').value = boardId;
    const boardName = getBoardName(boardId);
    document.getElementById('boardNameInput').value = boardName || 'Unnamed Board';
}

function saveCurrentBoard(boardId) {
    localStorage.setItem('kanbanBoardId', boardId);
    updateRecentBoards(boardId);
}

// Board Name Management
function getBoardNames() {
    const stored = localStorage.getItem('boardNames');
    return stored ? JSON.parse(stored) : {};
}

function getBoardName(boardId) {
    const boardNames = getBoardNames();
    return boardNames[boardId] || null;
}

function saveBoardName(nameOrEvent) {
    if (!currentBoardId) return;

    let boardName;
    if (typeof nameOrEvent === 'string') {
        boardName = nameOrEvent;
    } else {
        boardName = document.getElementById('boardNameInput').value.trim();
    }

    if (!boardName) {
        boardName = 'Unnamed Board';
        document.getElementById('boardNameInput').value = boardName;
    }

    const boardNames = getBoardNames();
    boardNames[currentBoardId] = boardName;
    localStorage.setItem('boardNames', JSON.stringify(boardNames));

    renderRecentBoards();
}

function updateRecentBoards(boardId) {
    let recentBoards = getRecentBoards();

    recentBoards = recentBoards.filter(id => id !== boardId);
    recentBoards.unshift(boardId);
    recentBoards = recentBoards.slice(0, MAX_RECENT_BOARDS);

    localStorage.setItem('recentBoards', JSON.stringify(recentBoards));
    renderRecentBoards();
}

function getRecentBoards() {
    const stored = localStorage.getItem('recentBoards');
    return stored ? JSON.parse(stored) : [];
}

function renderRecentBoards() {
    const recentBoards = getRecentBoards();
    const container = document.getElementById('recentBoardsList');

    if (!container) return;

    if (recentBoards.length === 0) {
        container.innerHTML = '<p class="no-boards">No recent boards</p>';
        return;
    }

    container.innerHTML = recentBoards.map((boardId, index) => {
        const boardName = getBoardName(boardId) || 'Unnamed Board';
        return `
            <div class="recent-board-item ${boardId === currentBoardId ? 'active' : ''}">
                <span class="board-number">${index + 1}.</span>
                <div class="board-info-item">
                    <span class="board-name" title="${escapeHtml(boardName)}">${escapeHtml(boardName)}</span>
                    <span class="board-id" title="${boardId}">${boardId.substring(0, 8)}...</span>
                </div>
                <button class="btn-small" onclick="switchToBoard('${boardId}')">Load</button>
                <button class="btn-small btn-danger" onclick="removeFromRecent('${boardId}')">Remove</button>
            </div>
        `;
    }).join('');
}

function switchToBoard(boardId) {
    document.getElementById('boardIdInput').value = boardId;
    loadBoard();
}

function removeFromRecent(boardId) {
    let recentBoards = getRecentBoards();
    recentBoards = recentBoards.filter(id => id !== boardId);
    localStorage.setItem('recentBoards', JSON.stringify(recentBoards));
    renderRecentBoards();
    showNotification('Board removed from recent list', 'info');
}

function initializeSortable() {
    const columns = ['todo-list', 'inprogress-list', 'done-list'];
    const statusMap = {
        'todo-list': 'ToDo',
        'inprogress-list': 'InProgress',
        'done-list': 'Done'
    };

    columns.forEach(columnId => {
        const element = document.getElementById(columnId);
        new Sortable(element, {
            group: 'kanban',
            animation: 150,
            ghostClass: 'task-ghost',
            dragClass: 'task-dragging',
            onEnd: async (evt) => {
                const taskId = evt.item.dataset.taskId;
                const newStatus = statusMap[evt.to.id];
                const newPosition = evt.newIndex;

                await moveTask(taskId, newStatus, newPosition);
            }
        });
    });
}

async function loadTasks() {
    if (!currentBoardId) return;

    showLoading(true);
    try {
        const response = await fetch(`${API_BASE}/Boards/${currentBoardId}/tasks`);

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const tasks = await response.json();
        renderTasks(tasks);
        showNotification(`Loaded ${tasks.length} tasks`, 'success');
    } catch (error) {
        console.error('Error loading tasks:', error);
        showNotification(`Failed to load tasks: ${error.message}`, 'error');
    } finally {
        showLoading(false);
    }
}

function renderTasks(tasks) {
    // Clear all columns
    clearBoard();

    // Group tasks by status
    const tasksByStatus = {
        'ToDo': [],
        'InProgress': [],
        'Done': []
    };

    tasks.forEach(task => {
        tasksByStatus[task.status].push(task);
    });

    // Render tasks in each column
    Object.keys(tasksByStatus).forEach(status => {
        const columnId = status.toLowerCase() + '-list';
        const column = document.getElementById(columnId);
        const taskList = tasksByStatus[status];

        taskList.forEach(task => {
            const taskElement = createTaskElement(task);
            column.appendChild(taskElement);
        });

        // Update task count
        updateTaskCount(status, taskList.length);
    });
}

function createTaskElement(task) {
    const div = document.createElement('div');
    div.className = 'task-card';
    div.dataset.taskId = task.id;

    div.innerHTML = `
        <div class="task-header">
            <h3 class="task-title">${escapeHtml(task.title)}</h3>
            <div class="task-actions">
                <button class="btn-icon" onclick="editTask('${task.id}')" title="Edit">✏️</button>
                <button class="btn-icon" onclick="deleteTask('${task.id}')" title="Delete">🗑️</button>
            </div>
        </div>
        ${task.description ? `<p class="task-description">${escapeHtml(task.description)}</p>` : ''}
        <div class="task-meta">
            <span class="task-id">#${task.id.substring(0, 8)}</span>
        </div>
    `;

    return div;
}

async function moveTask(taskId, newStatus, newPosition) {
    try {
        const response = await fetch(`${API_BASE}/Tasks/${taskId}/move`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                taskId: taskId,
                newStatus: newStatus,
                newPosition: newPosition
            })
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.title || 'Failed to move task');
        }

        showNotification('Task moved successfully', 'success');
        await loadTasks(); // Reload to sync positions
    } catch (error) {
        console.error('Error moving task:', error);
        showNotification(`Failed to move task: ${error.message}`, 'error');
        await loadTasks(); // Reload to revert UI
    }
}

function openTaskModal(task = null) {
    const modal = document.getElementById('taskModal');
    const title = document.getElementById('modalTitle');
    const form = document.getElementById('taskForm');

    if (task) {
        title.textContent = 'Edit Task';
        document.getElementById('taskTitle').value = task.title;
        document.getElementById('taskDescription').value = task.description || '';
        form.dataset.taskId = task.id;
    } else {
        title.textContent = 'Create New Task';
        form.reset();
        delete form.dataset.taskId;
    }

    modal.style.display = 'block';
}

function closeTaskModal() {
    document.getElementById('taskModal').style.display = 'none';
    document.getElementById('taskForm').reset();
}

async function handleTaskSubmit(e) {
    e.preventDefault();

    if (!currentBoardId) {
        showNotification('Please select or create a board first', 'error');
        return;
    }

    const form = e.target;
    const taskId = form.dataset.taskId;
    const title = document.getElementById('taskTitle').value;
    const description = document.getElementById('taskDescription').value;

    if (taskId) {
        await updateTask(taskId, title, description);
    } else {
        await createTask(title, description);
    }
}

async function createTask(title, description) {
    try {
        const response = await fetch(`${API_BASE}/Tasks`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                boardId: currentBoardId,
                title: title,
                description: description || null
            })
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.title || 'Failed to create task');
        }

        showNotification('Task created successfully', 'success');
        closeTaskModal();
        await loadTasks();
    } catch (error) {
        console.error('Error creating task:', error);
        showNotification(`Failed to create task: ${error.message}`, 'error');
    }
}

async function updateTask(taskId, title, description) {
    try {
        const response = await fetch(`${API_BASE}/Tasks/${taskId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                taskId: taskId,
                title: title,
                description: description || null
            })
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.title || 'Failed to update task');
        }

        showNotification('Task updated successfully', 'success');
        closeTaskModal();
        await loadTasks();
    } catch (error) {
        console.error('Error updating task:', error);
        showNotification(`Failed to update task: ${error.message}`, 'error');
    }
}

async function editTask(taskId) {
    try {
        const response = await fetch(`${API_BASE}/Tasks/${taskId}`);
        if (!response.ok) throw new Error('Task not found');

        const task = await response.json();
        openTaskModal(task);
    } catch (error) {
        console.error('Error loading task:', error);
        showNotification('Failed to load task', 'error');
    }
}

async function deleteTask(taskId) {
    if (!confirm('Are you sure you want to delete this task?')) return;

    try {
        const response = await fetch(`${API_BASE}/Tasks/${taskId}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error('Failed to delete task');
        }

        showNotification('Task deleted successfully', 'success');
        await loadTasks();
    } catch (error) {
        console.error('Error deleting task:', error);
        showNotification(`Failed to delete task: ${error.message}`, 'error');
    }
}

function clearBoard() {
    document.getElementById('todo-list').innerHTML = '';
    document.getElementById('inprogress-list').innerHTML = '';
    document.getElementById('done-list').innerHTML = '';
    updateTaskCount('ToDo', 0);
    updateTaskCount('InProgress', 0);
    updateTaskCount('Done', 0);
}

function updateTaskCount(status, count) {
    const counter = document.querySelector(`[data-column="${status}"]`);
    if (counter) {
        counter.textContent = count;
    }
}

function showLoading(show) {
    document.getElementById('loadingIndicator').style.display = show ? 'block' : 'none';
}

function showNotification(message, type = 'info') {
    // Simple notification - could be enhanced with a proper toast library
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.textContent = message;
    document.body.appendChild(notification);

    setTimeout(() => {
        notification.remove();
    }, 3000);
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
